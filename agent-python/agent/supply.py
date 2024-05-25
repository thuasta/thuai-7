from typing import Literal

from .position import Position

SupplyKind = Literal[
    "PRIMARY_ARMOR",
    "PREMIUM_ARMOR",
    "S686",
    "M16",
    "AWM",
    "VECTOR",
    "BANDAGE",
    "FIRST_AID",
    "BULLET",
    "GRENADE",
]


class Supply:
    def __init__(self, kind: SupplyKind, count: int, position: Position[float]):
        self.kind = kind
        self.count = count
        self.position = position

    def __str__(self) -> str:
        return f"Supply{{kind: {self.kind}, count: {self.count}, position: {self.position}}}"
