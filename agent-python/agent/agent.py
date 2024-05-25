import asyncio
import logging
from typing import List, Literal, Optional

from . import messages
from .grenade_info import GrenadeInfo
from .map import Map
from .player_info import FirearmKind, Item, ItemKind, PlayerInfo
from .position import Position
from .safe_zone import SafeZone
from .supply import Supply, SupplyKind
from .websocket_client import WebsocketClient

MedicineKind = Literal[
    "BANDAGE",
    "FIRST_AID",
]


class Agent:
    def __init__(self, token: str, loop_interval: float):
        self._token = token
        self._loop_interval = loop_interval

        self._all_player_info: Optional[List[PlayerInfo]] = None
        self._map: Optional[Map] = None
        self._supplies: Optional[List[Supply]] = None
        self._safe_zone: Optional[SafeZone] = None
        self._grenade_info: Optional[List[GrenadeInfo]] = None
        self._self_id: Optional[int] = None
        self._ticks: Optional[int] = None

        self._ws_client = WebsocketClient()
        self._loop_task: Optional[asyncio.Task] = None

        self._ws_client.on_message = self._on_message

    def __str__(self) -> str:
        return f"Agent{{token: {self._token}}}"

    def __repr__(self) -> str:
        return str(self)

    @property
    def all_player_info(self) -> Optional[List[PlayerInfo]]:
        return self._all_player_info

    @property
    def map(self) -> Optional[Map]:
        return self._map

    @property
    def supplies(self) -> Optional[List[Supply]]:
        return self._supplies

    @property
    def safe_zone(self) -> Optional[SafeZone]:
        return self._safe_zone

    @property
    def self_id(self) -> Optional[int]:
        return self._self_id

    @property
    def token(self) -> str:
        return self._token

    @property
    def ticks(self) -> Optional[int]:
        return self._ticks

    @property
    def grenade_info(self) -> Optional[List[GrenadeInfo]]:
        return self._grenade_info

    async def connect(self, server: str):
        await self._ws_client.connect(server)
        self._loop_task = asyncio.create_task(self._loop())

    async def disconnect(self):
        if self._loop_task is not None:
            self._loop_task.cancel()
        await self._ws_client.disconnect()

    def is_connected(self) -> bool:
        return self._ws_client.is_connected()

    def is_game_ready(self) -> bool:
        return (
            self._all_player_info is not None
            and self._map is not None
            and self._supplies is not None
            and self._safe_zone is not None
            and self._self_id is not None
            and self._ticks is not None
            and self._grenade_info is not None
        )

    async def abandon(self, supply: SupplyKind, count: int):
        logging.debug("%s.abandon(%s, %d)", self, supply, count)
        await self._ws_client.send(
            messages.PerformAbandonMessage(
                token=self._token,
                numb=count,
                target_supply=supply,
            )
        )

    async def pick_up(self, supply: SupplyKind, count: int):
        logging.debug("%s.pick_up(%s, %d)", self, supply, count)
        await self._ws_client.send(
            messages.PerformPickUpMessage(
                token=self._token, target_supply=supply, num=count
            )
        )

    async def switch_firearm(self, firearm: FirearmKind):
        logging.debug("%s.switch_firearm(%s)", self, firearm)
        await self._ws_client.send(
            messages.PerformSwitchArmMessage(
                token=self._token,
                target_firearm=firearm,
            )
        )

    async def use_medicine(self, medicine: MedicineKind):
        logging.debug("%s.use_medicine(%s)", self, medicine)
        await self._ws_client.send(
            messages.PerformUseMedicineMessage(
                token=self._token,
                medicine_name=medicine,
            )
        )

    async def use_grenade(self, position: Position[float]):
        logging.debug("%s.use_grenade(%s)", self, position)
        await self._ws_client.send(
            messages.PerformUseGrenadeMessage(
                token=self._token,
                target_position=messages.Position(x=position.x, y=position.y),
            )
        )

    async def move(self, position: Position[float]):
        logging.debug("%s.move(%s)", self, position)
        await self._ws_client.send(
            messages.PerformMoveMessage(
                token=self._token,
                destination=messages.Position(x=position.x, y=position.y),
            )
        )

    async def stop(self):
        logging.debug("%s.stop()", self)
        await self._ws_client.send(
            messages.PerformStopMessage(
                token=self._token,
            )
        )

    async def attack(self, position: Position[float]):
        logging.debug("%s.attack(%s)", self, position)
        await self._ws_client.send(
            messages.PerformAttackMessage(
                token=self._token,
                target_position=messages.Position(x=position.x, y=position.y),
            )
        )

    async def choose_origin(self, position: Position[float]):
        logging.debug("%s.choose_origin(%s)", self, position)
        await self._ws_client.send(
            messages.ChooseOriginMessage(
                token=self._token,
                origin_position=messages.Position(x=position.x, y=position.y),
            )
        )

    async def _loop(self):
        while True:
            try:
                await asyncio.sleep(self._loop_interval)

                if not self._ws_client.is_connected():
                    continue

                await self._ws_client.send(
                    messages.GetPlayerInfoMessage(
                        token=self._token,
                    )
                )

            except Exception as e:
                logging.error(f"{self} encountered an error in loop: {e}")

    def _on_message(self, message: messages.Message):
        try:
            msg_dict = message.msg
            msg_type = msg_dict["messageType"]

            if msg_type == "ERROR":
                logging.error(f"{self} got error from server: {msg_dict['message']}")

            elif msg_type == "PLAYERS_INFO":
                self._ticks = msg_dict["elapsedTicks"]
                self._all_player_info = [
                    PlayerInfo(
                        id=data["playerId"],
                        armor=data["armor"],
                        current_armor_health=data["current_armor_health"],
                        health=data["health"],
                        speed=data["speed"],
                        firearm=data["firearm"]["name"],
                        firearms_pool=[
                            weapon_data["name"] for weapon_data in data["firearms_pool"]
                        ],
                        range=data["firearm"]["distance"],
                        position=Position(
                            x=data["position"]["x"], y=data["position"]["y"]
                        ),
                        inventory=[
                            Item(kind=item["name"], count=item["num"])
                            for item in data["inventory"]
                        ],
                    )
                    for data in msg_dict["players"]
                ]

            elif msg_type == "MAP":
                self._map = Map(
                    length=msg_dict["length"],
                    obstacles=[
                        Position(
                            x=wall["wallPositions"]["x"], y=wall["wallPositions"]["y"]
                        )
                        for wall in msg_dict["walls"]
                    ],
                )

            elif msg_type == "SUPPLIES":
                self._supplies = [
                    Supply(
                        kind=supply["name"],
                        position=Position(
                            x=supply["position"]["x"], y=supply["position"]["y"]
                        ),
                        count=supply["numb"],
                    )
                    for supply in msg_dict["supplies"]
                ]

            elif msg_type == "SAFE_ZONE":
                self._safe_zone = SafeZone(
                    center=Position(
                        x=msg_dict["center"]["x"], y=msg_dict["center"]["y"]
                    ),
                    radius=msg_dict["radius"],
                )

            elif msg_type == "PLAYER_ID":
                self._self_id = msg_dict["playerId"]

            elif msg_type == "GRENADES":
                self._grenade_info = [
                    GrenadeInfo(
                        throwTick=grenade["throwTick"],
                        evaluatedPosition=Position(
                            x=grenade["evaluatedPosition"]["x"],
                            y=grenade["evaluatedPosition"]["y"],
                        ),
                    )
                    for grenade in msg_dict["grenades"]
                ]

        except Exception as e:
            logging.error(f"{self} failed to handle message: {e}")
