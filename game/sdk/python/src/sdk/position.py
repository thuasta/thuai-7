"""The position.
"""

from dataclasses import dataclass
from typing import Generic, TypeVar

T = TypeVar("T")


@dataclass
class Position(Generic[T]):
    """The position.

    Attributes:
        x: The x coordinate.
        y: The y coordinate.
        z: The z coordinate.
    """
    x: T
    y: T
    z: T

    def __hash__(self) -> int:
        return hash((self.x, self.y, self.z))
