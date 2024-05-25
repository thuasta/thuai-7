#ifndef THUAI7_AGENT_SAFE_ZONE_H_
#define THUAI7_AGENT_SAFE_ZONE_H_

#include <string>

#include "position.h"

namespace thuai7_agent {

struct SafeZone {
  Position<float> center;
  float radius;
};

auto format_as(SafeZone const& object) -> std::string;

}  // namespace thuai7_agent

#endif  // THUAI7_AGENT_SAFE_ZONE_H_
