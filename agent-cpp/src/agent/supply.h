#ifndef THUAI7_AGENT_SUPPLY_H_
#define THUAI7_AGENT_SUPPLY_H_

#include <string>

#include "position.h"

namespace thuai7_agent {

enum class SupplyKind {
  kS686,
  kVector,
  kAwm,
  kM16,
  kBullet,
  kBandage,
  kPrimaryArmor,
  kPremiumArmor,
  kFirstAid,
  kGrenade,
};
struct Supply {
  SupplyKind kind;
  int count;
  Position<float> position;
};

auto format_as(SupplyKind object) -> std::string;
auto format_as(Supply const& object) -> std::string;

}  // namespace thuai7_agent

#endif  // THUAI7_AGENT_SUPPLY_H_
