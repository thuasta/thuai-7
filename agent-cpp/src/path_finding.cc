#include "path_finding.h"

#include <algorithm>
#include <array>
#include <cmath>
#include <functional>
#include <queue>
#include <unordered_map>
#include <unordered_set>
#include <vector>

#include "agent/position.h"

static auto CalculateManhattanDistance(thuai7_agent::Position<int> const& start,
                                       thuai7_agent::Position<int> const& end)
    -> int;
static auto GetNeighbors(thuai7_agent::Map const& map,
                         thuai7_agent::Position<int> const& position)
    -> std::vector<thuai7_agent::Position<int>>;
static auto IsValidPosition(thuai7_agent::Map const& map,
                            thuai7_agent::Position<int> const& position)
    -> bool;

auto FindPathBeFS(thuai7_agent::Map const& map,
                  thuai7_agent::Position<int> const& start,
                  thuai7_agent::Position<int> const& end)
    -> std::vector<thuai7_agent::Position<int>> {
  auto priority_queue_comparator =
      [&end](thuai7_agent::Position<int> const& lhs,
             thuai7_agent::Position<int> const& rhs) -> bool {
    return CalculateManhattanDistance(lhs, end) >
           CalculateManhattanDistance(rhs, end);
  };
  auto unordered_set_hasher =
      [](thuai7_agent::Position<int> const& position) -> std::size_t {
    return std::hash<int>{}(position.x) ^ std::hash<int>{}(position.y);
  };

  std::priority_queue<thuai7_agent::Position<int>,
                      std::vector<thuai7_agent::Position<int>>,
                      decltype(priority_queue_comparator)>
      queue(priority_queue_comparator);

  std::unordered_set<thuai7_agent::Position<int>,
                     decltype(unordered_set_hasher)>
      visited(0, unordered_set_hasher);

  std::unordered_map<thuai7_agent::Position<int>, thuai7_agent::Position<int>,
                     decltype(unordered_set_hasher)>
      parents(0, unordered_set_hasher);

  queue.push(start);
  visited.insert(start);

  bool is_found = false;

  while (!queue.empty() && !is_found) {
    auto current = queue.top();
    queue.pop();

    if (current == end) {
      is_found = true;
      break;
    }

    for (auto const& neighbor : GetNeighbors(map, current)) {
      if (visited.find(neighbor) != visited.end()) {
        continue;
      }

      queue.push(neighbor);
      visited.insert(neighbor);
      parents[neighbor] = current;
    }
  }

  if (!is_found) {
    return {};
  }

  std::vector<thuai7_agent::Position<int>> path;
  for (auto current = end; current != start; current = parents[current]) {
    path.push_back(current);
  }
  path.push_back(start);

  std::reverse(path.begin(), path.end());

  return path;
}

static auto CalculateManhattanDistance(thuai7_agent::Position<int> const& start,
                                       thuai7_agent::Position<int> const& end)
    -> int {
  return std::abs(start.x - end.x) + std::abs(start.y - end.y);
}

static auto GetNeighbors(thuai7_agent::Map const& map,
                         thuai7_agent::Position<int> const& position)
    -> std::vector<thuai7_agent::Position<int>> {
  std::array<thuai7_agent::Position<int>, 4> neighbors{
      thuai7_agent::Position<int>{position.x, position.y - 1},
      thuai7_agent::Position<int>{position.x + 1, position.y},
      thuai7_agent::Position<int>{position.x, position.y + 1},
      thuai7_agent::Position<int>{position.x - 1, position.y},
  };

  std::vector<thuai7_agent::Position<int>> valid_neighbors;
  for (auto const& neighbor : neighbors) {
    if (IsValidPosition(map, neighbor)) {
      valid_neighbors.push_back(neighbor);
    }
  }

  return valid_neighbors;
}

static auto IsValidPosition(thuai7_agent::Map const& map,
                            thuai7_agent::Position<int> const& position)
    -> bool {
  for (auto const& obstacle : map.obstacles) {
    if (position == obstacle) {
      return false;
    }
  }

  return position.x >= 0 && position.x < map.length && position.y >= 0 &&
         position.y < map.length;
}
