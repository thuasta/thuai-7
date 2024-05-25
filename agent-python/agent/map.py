from typing import List

from .position import Position


class Map:
    def __init__(self, length: int, obstacles: List[Position[int]]):
        self.length = length
        self.obstacles = obstacles

    def __str__(self) -> str:
        return f"Map{{length: {self.length}, obstacles: {self.obstacles}}}"
