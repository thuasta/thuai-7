from .position import Position


class SafeZone:
    def __init__(self, center: Position[float], radius: float):
        self.center = center
        self.radius = radius

    def __str__(self) -> str:
        return f"SafeZone{{center: {self.center}, radius: {self.radius}}}"
