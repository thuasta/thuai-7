"""The entity APIs.

The entity API is the API that can be used to get information about entities.
"""

from abc import ABC, abstractmethod

from .orientation import Orientation
from .position import Position


class IEntity(ABC):
    """The entity interface.

    The entity interface is the interface that can be used to get
    information about entities.
    """
    @abstractmethod
    def get_orientation(self) -> Orientation:
        """Gets the orientation of the entity.

        Returns:
            The orientation.
        """
        raise NotImplementedError()

    @abstractmethod
    def get_position(self) -> Position[float]:
        """Gets the position of the entity.

        Returns:
            The position.
        """
        raise NotImplementedError()

    @abstractmethod
    def get_type_id(self) -> int:
        """Gets the type ID of the entity.

        Returns:
            The type ID.
        """
        raise NotImplementedError()

    @abstractmethod
    def get_unique_id(self) -> int:
        """Gets the unique ID of the entity.

        Returns:
            The unique ID.
        """
        raise NotImplementedError()


class Entity(IEntity):
    def __init__(self, type_id: int, unique_id: int, position: Position[float], orientation: Orientation[float]):
        super().__init__()

        self._orientation = orientation
        self._position = position
        self._type_id = type_id
        self._unique_id = unique_id

    def get_orientation(self) -> Orientation[float]:
        return self._orientation
    
    def get_position(self) -> Position[float]:
        return self._position
    
    def get_type_id(self) -> int:
        return self._type_id
    
    def get_unique_id(self) -> int:
        return self._unique_id
    
    def set_orientation(self, orientation: Orientation[float]):
        self._orientation = orientation

    def set_position(self, position: Position[float]):
        self._position = position
