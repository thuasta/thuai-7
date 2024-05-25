#include "supply.h"

#include <fmt/format.h>

#include <magic_enum.hpp>
#include <string>

namespace thuai7_agent {

auto format_as(SupplyKind object) -> std::string {
  return std::string(magic_enum::enum_name(object));
}

auto format_as(Supply const& object) -> std::string {
  return fmt::format("Supply{{kind: {}, count: {}, position: {}}}", object.kind,
                     object.count, object.position);
}

}  // namespace thuai7_agent
