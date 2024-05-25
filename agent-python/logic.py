import logging
from typing import List, Optional

from pathfinding.core.grid import Grid
from pathfinding.finder.best_first import BestFirst

from agent.agent import Agent
from agent.position import Position

game_map_matrix: Optional[List[List[int]]] = None
path: List[Position[int]] = []


async def setup(agent: Agent):
    # Your code here.
    pass


async def loop(agent: Agent):
    # Your code here.
    # Here is an example of how to use the agent.
    # Always move to the opponent's position, keep one cell away from the
    # opponent, and attack the opponent.

    player_info_list = agent.all_player_info
    assert player_info_list is not None

    self_id = agent.self_id
    assert self_id is not None

    self_info = player_info_list[self_id]
    opponent_info = player_info_list[(self_id + 1) % len(player_info_list)]

    game_map = agent.map
    assert game_map is not None

    global game_map_matrix

    if game_map_matrix is None:
        game_map_matrix = [
            [1 for _ in range(game_map.length)] for _ in range(game_map.length)
        ]
        for obstacle in game_map.obstacles:
            game_map_matrix[obstacle.y][obstacle.x] = 0

    self_position_int = Position[int](
        int(self_info.position.x), int(self_info.position.y)
    )
    opponent_position_int = Position[int](
        int(opponent_info.position.x), int(opponent_info.position.y)
    )

    global path

    if self_position_int not in path or opponent_position_int not in path:
        path = find_path_befs(game_map_matrix, self_position_int, opponent_position_int)

        if len(path) == 0:
            logging.info(
                "no path from %s to %s", self_position_int, opponent_position_int
            )
            return

        logging.info(f"found path from {self_position_int} to {opponent_position_int}")

    while path[0] != self_position_int:
        path.pop(0)

    if len(path) > 1:
        next_position_int = path[1]
        next_position = Position[float](
            float(next_position_int.x) + 0.5, float(next_position_int.y) + 0.5
        )

        await agent.move(next_position)
        return

    await agent.attack(opponent_info.position)


def find_path_befs(
    game_map_matrix: List[List[int]], start: Position[int], end: Position[int]
) -> List[Position[int]]:
    game_map_grid = Grid(matrix=game_map_matrix)
    start_node = game_map_grid.node(start.x, start.y)
    end_node = game_map_grid.node(end.x, end.y)
    finder = BestFirst()
    path, _ = finder.find_path(start_node, end_node, game_map_grid)
    assert isinstance(path, list)
    return [Position[int](x, y) for x, y in path]
