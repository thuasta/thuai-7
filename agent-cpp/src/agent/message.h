#pragma once
#ifndef THUAI7_AGENT_MESSAGE_H_
#define THUAI7_AGENT_MESSAGE_H_

// std c++ libraries
#include <string>
// 3rd party libraries
#include <nlohmann/json.hpp>
// project headers
#include "agent/player_info.h"
#include "agent/position.h"
#include "agent/supply.h"

namespace thuai7_agent {

NLOHMANN_JSON_SERIALIZE_ENUM(SupplyKind,
                             {
                                 {SupplyKind::kS686, "S686"},
                                 {SupplyKind::kVector, "VECTOR"},
                                 {SupplyKind::kAwm, "AWM"},
                                 {SupplyKind::kM16, "M16"},
                                 {SupplyKind::kBullet, "BULLET"},
                                 {SupplyKind::kBandage, "BANDAGE"},
                                 {SupplyKind::kPrimaryArmor, "PRIMARY_ARMOR"},
                                 {SupplyKind::kPremiumArmor, "PREMIUM_ARMOR"},
                                 {SupplyKind::kFirstAid, "FIRST_AID"},
                                 {SupplyKind::kGrenade, "GRENADE"},
                             });
NLOHMANN_JSON_SERIALIZE_ENUM(ItemKind, {
                                           {ItemKind::kBandage, "BANDAGE"},
                                           {ItemKind::kFirstAid, "FIRST_AID"},
                                           {ItemKind::kBullet, "BULLET"},
                                           {ItemKind::kGrenade, "GRENADE"},
                                       });

NLOHMANN_JSON_SERIALIZE_ENUM(FirearmKind, {
                                              {FirearmKind::kFist, "FIST"},
                                              {FirearmKind::kS686, "S686"},
                                              {FirearmKind::kVector, "VECTOR"},
                                              {FirearmKind::kAwm, "AWM"},
                                              {FirearmKind::kM16, "M16"},
                                          });

NLOHMANN_JSON_SERIALIZE_ENUM(MedicineKind,
                             {
                                 {MedicineKind::kBandage, "BANDAGE"},
                                 {MedicineKind::kFirstAid, "FIRST_AID"},
                             });

NLOHMANN_JSON_SERIALIZE_ENUM(ArmorKind,
                             {
                                 {ArmorKind::kNone, "NO_ARMOR"},
                                 {ArmorKind::kPrimary, "PRIMARY_ARMOR"},
                                 {ArmorKind::kPremium, "PREMIUM_ARMOR"},
                             });

struct Message {
  explicit Message(const std::string& json_str)
      : msg(nlohmann::json::parse(json_str)) {}
  Message() = default;

  [[nodiscard]] auto json() const -> std::string { return msg.dump(); }

  nlohmann::json msg;
};

struct PerformAbandonMessage : public Message {
  PerformAbandonMessage(int numb, const std::string& token,
                        const SupplyKind& target_supply) {
    msg["messageType"] = "PERFORM_ABANDON";
    msg["numb"] = numb;
    msg["token"] = token;
    msg["targetSupply"] = target_supply;
  }
};

struct PerformPickUpMessage : public Message {
  PerformPickUpMessage(const std::string& token,
                       const SupplyKind& target_supply, int num) {
    msg["messageType"] = "PERFORM_PICK_UP";
    msg["token"] = token;
    msg["targetSupply"] = target_supply;
    msg["num"] = num;
  }
};

struct PerformSwitchArmMessage : public Message {
  PerformSwitchArmMessage(const std::string& token,
                          const FirearmKind& target_firearm) {
    msg["messageType"] = "PERFORM_SWITCH_ARM";
    msg["token"] = token;
    msg["targetFirearm"] = target_firearm;
  }
};

struct PerformUseMedicineMessage : public Message {
  PerformUseMedicineMessage(const std::string& token,
                            const MedicineKind& medicine_name) {
    msg["messageType"] = "PERFORM_USE_MEDICINE";
    msg["token"] = token;
    msg["medicineName"] = medicine_name;
  }
};

struct PerformUseGrenadeMessage : public Message {
  PerformUseGrenadeMessage(const std::string& token,
                           const Position<float>& target_position) {
    msg["messageType"] = "PERFORM_USE_GRENADE";
    msg["token"] = token;
    msg["targetPosition"] = {{"x", target_position.x},
                             {"y", target_position.y}};
  }
};

struct PerformMoveMessage : public Message {
  PerformMoveMessage(const std::string& token,
                     const Position<float>& destination) {
    msg["messageType"] = "PERFORM_MOVE";
    msg["token"] = token;
    msg["destination"] = {{"x", destination.x}, {"y", destination.y}};
  }
};

struct PerformStopMessage : public Message {
  explicit PerformStopMessage(const std::string& token) {
    msg["messageType"] = "PERFORM_STOP";
    msg["token"] = token;
  }
};

struct PerformAttackMessage : public Message {
  PerformAttackMessage(const std::string& token,
                       const Position<float>& target_position) {
    msg["messageType"] = "PERFORM_ATTACK";
    msg["token"] = token;
    msg["targetPosition"] = {{"x", target_position.x},
                             {"y", target_position.y}};
  }
};

struct GetPlayerInfoMessage : public Message {
  explicit GetPlayerInfoMessage(const std::string& token) {
    msg["messageType"] = "GET_PLAYER_INFO";
    msg["token"] = token;
  }
};

struct GrenadeMessage : public Message {
  explicit GrenadeMessage(const std::string& token) {
    msg["messageType"] = "GRENADE_MESSAGE";
    msg["token"] = token;
  }
};

struct GetMapMessage : public Message {
  explicit GetMapMessage(const std::string& token) {
    msg["messageType"] = "GET_MAP";
    msg["token"] = token;
  }
};

struct ChooseOriginMessage : public Message {
  ChooseOriginMessage(const std::string& token,
                      const Position<float>& origin_position) {
    msg["messageType"] = "CHOOSE_ORIGIN";
    msg["token"] = token;
    msg["originPosition"] = {{"x", origin_position.x},
                             {"y", origin_position.y}};
  }
};

}  // namespace thuai7_agent

#endif  // THUAI7_AGENT_MESSAGE_H_
