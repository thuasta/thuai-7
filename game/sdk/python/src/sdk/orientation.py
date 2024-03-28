"""The orientation.
"""

from dataclasses import dataclass
from typing import Generic, TypeVar

T = TypeVar("T")


@dataclass
class Orientation(Generic[T]):
    """The orientation.

    Attributes:
        yaw: The yaw.
        pitch: The pitch.
    """
    yaw: T
    pitch: T

    def __hash__(self) -> int:
        return hash((self.yaw, self.pitch))