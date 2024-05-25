#include "grenade_info.h"

#include <fmt/format.h>

#include <string>

namespace thuai7_agent {

auto format_as(GrenadeInfo const& object) -> std::string {
  return fmt::format("GrenadeInfo{{throwTick: {}, evaluatedPosition: {}}}",
                     object.throwTick, object.evaluatedPosition);
}

}  // namespace thuai7_agent
