#ifndef THUAI7_AGENT_GRENADE_INFO_H_
#define THUAI7_AGENT_GRENADE_INFO_H_

#include <string>

#include "position.h"

namespace thuai7_agent {

struct GrenadeInfo {
  int throwTick;
  Position<float> evaluatedPosition;
};

auto format_as(GrenadeInfo const& object) -> std::string;

}  // namespace thuai7_agent

#endif  // THUAI7_AGENT_GRENADE_INFO_H_
