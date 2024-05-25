#include <fmt/ranges.h>
#include <spdlog/spdlog.h>

#include <vector>

#include "agent/agent.h"
#include "agent/position.h"
#include "path_finding.h"
#include "shot_check.h"

constexpr auto kFloatPositionShift = 0.5;

// NOLINTBEGIN(cppcoreguidelines-avoid-non-const-global-variables)
static struct {
  std::vector<thuai7_agent::Position<int>> path;
} state;
// NOLINTEND(cppcoreguidelines-avoid-non-const-global-variables)

void Setup(thuai7_agent::Agent& agent) {
  // Your code here.
}

void Loop(thuai7_agent::Agent& agent) {
  // Your code here.
  // Here is an example of how to use the agent.
  // Always move to the opponent's position, keep one cell away from the
  // opponent, and attack the opponent.

  auto const& player_info_list = agent.all_player_info().value().get();
  auto const self_id = agent.self_id().value();
  auto const& self_info = player_info_list.at(self_id);
  auto const& opponent_info =
      player_info_list.at((self_info.id + 1) % player_info_list.size());

  auto const& map = agent.map().value().get();

  auto self_position_int =
      thuai7_agent::Position<int>{static_cast<int>(self_info.position.x),
                                  static_cast<int>(self_info.position.y)};
  auto opponent_position_int =
      thuai7_agent::Position<int>{static_cast<int>(opponent_info.position.x),
                                  static_cast<int>(opponent_info.position.y)};

  if (std::find(state.path.begin(), state.path.end(), self_position_int) ==
          state.path.end() ||
      std::find(state.path.begin(), state.path.end(), opponent_position_int) ==
          state.path.end()) {
    state.path = FindPathBeFS(map, self_position_int, opponent_position_int);

    if (state.path.empty()) {
      spdlog::info("no path from {} to {}", self_position_int,
                   opponent_position_int);
      return;
    }

    spdlog::info("found path from {} to {}", self_position_int,
                 opponent_position_int);
  }

  while (state.path.front() != self_position_int) {
    state.path.erase(state.path.begin());
  }

  if (state.path.size() > 1) {
    auto next_position_int = state.path.at(1);
    auto next_position = thuai7_agent::Position<float>{
        static_cast<float>(next_position_int.x + kFloatPositionShift),
        static_cast<float>(next_position_int.y + kFloatPositionShift)};

    agent.Move(next_position);
    return;
  }

  if (CheckShotFeasible(map, self_info.position, opponent_info.position,
                        self_info.range)) {
    agent.Attack(opponent_info.position);
  }
}
