#ifndef THUAI7_AGENT_MAP_H_
#define THUAI7_AGENT_MAP_H_

#include <string>
#include <vector>

#include "position.h"

namespace thuai7_agent {

struct Map {
  int length;
  std::vector<Position<int>> obstacles;
};

auto format_as(Map const& object) -> std::string;

}  // namespace thuai7_agent

#endif  // THUAI7_AGENT_MAP_H_
