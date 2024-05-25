#include "player_info.h"

#include <fmt/ranges.h>

#include <magic_enum.hpp>
#include <string>

namespace thuai7_agent {

auto format_as(ArmorKind object) -> std::string {
  return std::string(magic_enum::enum_name(object));
}

auto format_as(FirearmKind object) -> std::string {
  return std::string(magic_enum::enum_name(object));
}

auto format_as(MedicineKind object) -> std::string {
  return std::string(magic_enum::enum_name(object));
}

auto format_as(ItemKind object) -> std::string {
  return std::string(magic_enum::enum_name(object));
}

auto format_as(Item const& object) -> std::string {
  return fmt::format("Item{{kind: {}, count: {}}}", object.kind, object.count);
}

auto format_as(PlayerInfo const& object) -> std::string {
  return fmt::format(
      "PlayerInfo{{id: {}, armor: {}, current_armor_health: {}, health: {}, "
      "speed: {}, firearm: {}, firearms_pool:{}, range: {}, "
      "position: {}, inventory: {}}}",
      object.id, object.armor, object.current_armor_health, object.health,
      object.speed, object.firearm, object.firearms_pool, object.range,
      object.position, object.inventory);
}

}  // namespace thuai7_agent
