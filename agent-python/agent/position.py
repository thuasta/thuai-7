from typing import Generic, TypeVar

T = TypeVar("T", int, float)


class Position(Generic[T]):
    def __init__(self, x: T, y: T):
        self.x = x
        self.y = y

    def __eq__(self, value: "Position[T]") -> bool:
        return self.x == value.x and self.y == value.y

    def __str__(self) -> str:
        return f"Position{{x: {self.x}, y: {self.y}}}"

    def __repr__(self) -> str:
        return str(self)
