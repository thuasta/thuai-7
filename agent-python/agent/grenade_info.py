from typing import List, Literal

from .position import Position


class GrenadeInfo:
    def __init__(
        self,
        throwTick: int,
        evaluatedPosition: Position,
    ):
        self.throwTick = throwTick
        self.evaluatedPosition = evaluatedPosition

    def __str__(self) -> str:
        return f"GrenadeInfo{{throwTick: {self.throwTick}, evaluatedPosition: {self.evaluatedPosition}}}"
