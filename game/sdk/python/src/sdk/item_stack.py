"""The item APIs.

The item API is the API that can be used to get information about items.
"""

from abc import ABC, abstractmethod


class IItemStack(ABC):
    """The item stack interface.
    
    The item stack interface is the interface that can be used to get
    information about item stacks.
    """
    @abstractmethod
    def get_count(self) -> int:
        """Gets the count of the item stack.
        
        Returns:
            The count.
        """
        raise NotImplementedError()

    @abstractmethod
    def get_type_id(self) -> int:
        """Gets the type ID of the item stack.
        
        Returns:
            The type ID.
        """
        raise NotImplementedError()
    
class ItemStack(IItemStack):
    def __init__(self, type_id: int, count: int):
        super().__init__()
        
        self._count = count
        self._type_id = type_id

    def get_count(self) -> int:
        return self._count
    
    def get_type_id(self) -> int:
        return self._type_id
    
    def set_count(self, count: int) -> None:
        self._count = count
