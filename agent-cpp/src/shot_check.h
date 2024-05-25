#ifndef SHOT_CHECK_H_
#define SHOT_CHECK_H_

#include "agent/map.h"
#include "agent/position.h"

auto CheckShotFeasible(thuai7_agent::Map const& map,
                       thuai7_agent::Position<float> const& shooter_position,
                       thuai7_agent::Position<float> const& target_position,
                       float shot_range) -> bool;

#endif  // SHOT_CHECK_H_