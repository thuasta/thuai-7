#include "shot_check.h"

#include <cmath>
#include <vector>

#include "agent/map.h"
#include "agent/position.h"

constexpr float kSearchStep = 0.1F;

auto CheckShotFeasible(thuai7_agent::Map const& map,
                       thuai7_agent::Position<float> const& shooter_position,
                       thuai7_agent::Position<float> const& target_position,
                       float shot_range) -> bool {
  auto const delta_x = target_position.x - shooter_position.x;
  auto const delta_y = target_position.y - shooter_position.y;
  auto const distance = std::sqrt(delta_x * delta_x + delta_y * delta_y);

  if (distance > shot_range) {
    return false;
  }

  auto map_grid = std::vector<std::vector<bool>>(
      map.length, std::vector<bool>(map.length, false));
  for (auto const& obstacle : map.obstacles) {
    map_grid.at(obstacle.x).at(obstacle.y) = true;
  }

  for (float delta_distance = 0.0F; delta_distance < distance;
       delta_distance += kSearchStep) {
    auto const current_position = thuai7_agent::Position<float>{
        shooter_position.x + delta_x * delta_distance / distance,
        shooter_position.y + delta_y * delta_distance / distance};

    auto const current_grid_position =
        thuai7_agent::Position<int>{static_cast<int>(current_position.x),
                                    static_cast<int>(current_position.y)};
    if (map_grid.at(current_grid_position.x).at(current_grid_position.y)) {
      return false;
    }
  }

  return true;
}
