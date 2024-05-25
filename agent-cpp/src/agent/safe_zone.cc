#include "safe_zone.h"

#include <fmt/format.h>

#include <string>

namespace thuai7_agent {

auto format_as(SafeZone const& object) -> std::string {
  return fmt::format("SafeZone{{center: {}, radius: {}}}", object.center,
                     object.radius);
}

}  // namespace thuai7_agent
