from typing import List, Literal

from .position import Position

ArmorKind = Literal[
    "NO_ARMOR",
    "PRIMARY_ARMOR",
    "PREMIUM_ARMOR",
]

FirearmKind = Literal[
    "FIST",
    "S686",
    "M16",
    "AWM",
    "VECTOR",
]

ItemKind = Literal[
    "BANDAGE",
    "FIRST_AID",
    "BULLET",
    "GRENADE",
]


class Item:
    def __init__(self, kind: ItemKind, count: int):
        self.kind = kind
        self.count = count


class PlayerInfo:
    def __init__(
        self,
        id: int,
        armor: ArmorKind,
        current_armor_health: float,
        health: int,
        speed: float,
        firearm: FirearmKind,
        firearms_pool: List[FirearmKind],
        range: float,
        position: Position[float],
        inventory: List[Item],
    ):
        self.id = id
        self.armor = armor
        self.current_armor_health = current_armor_health
        self.health = health
        self.speed = speed
        self.firearm = firearm
        self.firearms_pool = firearms_pool
        self.range = range
        self.position = position
        self.inventory = inventory

    def __str__(self) -> str:
        return f"PlayerInfo{{id: {self.id}, armor: {self.armor}, current_armor_health:{self.current_armor_health}, health: {self.health}, speed: {self.speed}, firearm: {self.firearm}, firearms_pool:{self.firearms_pool}, range: {self.range}, position: {self.position}, inventory: {self.inventory}}}"
