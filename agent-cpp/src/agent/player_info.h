#ifndef THUAI7_AGENT_PLAYER_INFO_H_
#define THUAI7_AGENT_PLAYER_INFO_H_

#include <string>
#include <vector>

#include "position.h"

namespace thuai7_agent {

enum class ArmorKind {
  kNone,
  kPrimary,
  kPremium,
};

enum class FirearmKind {
  kFist,
  kAwm,
  kM16,
  kS686,
  kVector,
};

enum class MedicineKind {
  kBandage,
  kFirstAid,
};

enum class ItemKind {
  kBandage,
  kBullet,
  kFirstAid,
  kGrenade,
};

struct Item {
  ItemKind kind;
  int count;
};

struct PlayerInfo {
  int id;
  ArmorKind armor;
  float current_armor_health;
  int health;
  float speed;
  FirearmKind firearm;
  std::vector<FirearmKind> firearms_pool;
  float range;
  Position<float> position;
  std::vector<Item> inventory;
};

auto format_as(ArmorKind object) -> std::string;
auto format_as(FirearmKind object) -> std::string;
auto format_as(MedicineKind object) -> std::string;
auto format_as(ItemKind object) -> std::string;
auto format_as(Item const& object) -> std::string;
auto format_as(PlayerInfo const& object) -> std::string;

}  // namespace thuai7_agent

#endif  // THUAI7_AGENT_PLAYER_INFO_H_
