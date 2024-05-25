import asyncio
import logging
from dataclasses import dataclass
from typing import Callable, Optional, Tuple

import websockets

from . import messages

RECONNECT_INTERVAL = 3.0


@dataclass
class _Connection:
    ws_client: websockets.WebSocketClientProtocol
    receive_task: asyncio.Task


class WebsocketClient:
    def __init__(self):
        self._connection: Optional[_Connection] = None
        self._on_message: Callable[[messages.Message], None] = lambda _: None

    @property
    def on_message(self) -> Callable[[messages.Message], None]:
        return self._on_message

    @on_message.setter
    def on_message(self, handler: Callable[[messages.Message], None]):
        self._on_message = handler

    async def connect(self, server_address: str):
        if self._connection is not None:
            await self.disconnect()

        assert self._connection is None

        ws_client = await self._try_connect(server_address)
        task = asyncio.create_task(self._receive_loop())
        self._connection = _Connection(ws_client, task)

    async def disconnect(self):
        if self._connection is not None:
            self._connection.receive_task.cancel()
            await self._connection.ws_client.close()
            self._connection = None

    def is_connected(self) -> bool:
        return self._connection is not None

    async def send(self, message: messages.Message):
        try:
            if self._connection is None:
                raise ValueError("connection is not established")

            await self._connection.ws_client.send(message.json())
            await asyncio.sleep(0.001)

        except Exception as e:
            logging.error("failed to send message to server: %s", e)

    async def _receive_loop(self):
        while True:
            await asyncio.sleep(0)
            try:
                connection = self._connection
                if connection is None:
                    continue

                try:
                    json_string = await connection.ws_client.recv()

                except Exception as e:
                    logging.error(f"failed to receive message from server: {e}")
                    logging.info("reconnecting...")
                    remote_address: Tuple[str, int] = (
                        connection.ws_client.remote_address
                    )
                    await connection.ws_client.close()
                    connection.ws_client = await self._try_connect(
                        f"ws://{remote_address[0]}:{remote_address[1]}"
                    )
                    self._connection = connection
                    continue

                try:
                    message = messages.Message(str(json_string))

                except Exception as e:
                    logging.error(f"failed to parse message from server: {e}")
                    continue

                self._on_message(message)

            except Exception as e:
                logging.error(f"encountered an error in receive loop: {e}")

    @staticmethod
    async def _try_connect(url: str) -> websockets.WebSocketClientProtocol:
        is_connected = False

        while not is_connected:
            try:
                return await websockets.connect(url)

            except Exception as e:
                logging.error(f"failed to connect to {url}: {e}")
                await asyncio.sleep(RECONNECT_INTERVAL)
                logging.info("retrying...")

        raise RuntimeError("unreachable")
