#ifndef THUAI7_AGENT_AGENT_H_
#define THUAI7_AGENT_AGENT_H_

#include <hv/EventLoop.h>
#include <hv/WebSocketClient.h>

#include <functional>
#include <memory>
#include <optional>
#include <string>
#include <string_view>
#include <vector>

#include "grenade_info.h"
#include "hv/Event.h"
#include "map.h"
#include "message.h"
#include "player_info.h"
#include "position.h"
#include "safe_zone.h"
#include "supply.h"

namespace thuai7_agent {

class Agent {
 public:
  explicit Agent(std::string_view token, hv::EventLoopPtr const& event_loop,
                 int loop_interval);

  Agent(Agent const&) = delete;
  Agent(Agent&&) = default;
  auto operator=(Agent const&) -> Agent& = delete;
  auto operator=(Agent&&) -> Agent& = default;
  ~Agent();

  // Methods for interacting with the server.

  [[nodiscard]] auto token() const -> std::string { return token_; }

  [[nodiscard]] auto IsConnected() const -> bool;

  void Connect(std::string_view server_address);

  void Disconnect();

  // Methods for interacting with the game.

  [[nodiscard]] auto all_player_info() const
      -> std::optional<std::reference_wrapper<std::vector<PlayerInfo> const>> {
    return all_player_info_;
  }

  [[nodiscard]] auto map() const
      -> std::optional<std::reference_wrapper<Map const>> {
    return map_;
  }

  [[nodiscard]] auto supplies() const
      -> std::optional<std::reference_wrapper<std::vector<Supply> const>> {
    return supplies_;
  }

  [[nodiscard]] auto safe_zone() const
      -> std::optional<std::reference_wrapper<SafeZone const>> {
    return safe_zone_;
  }

  [[nodiscard]] auto grenade_info() const
      -> std::optional<std::reference_wrapper<std::vector<GrenadeInfo> const>> {
    return grenade_info_;
  }

  [[nodiscard]] auto self_id() const -> std::optional<int> { return self_id_; }

  [[nodiscard]] auto ticks() const -> std::optional<int> { return ticks_; }

  [[nodiscard]] auto IsGameReady() const -> bool;

  void Abandon(SupplyKind target_supply, int count);

  void PickUp(SupplyKind target_supply, int count);

  void SwitchFirearm(FirearmKind target_firearm);

  void UseMedicine(MedicineKind target_medicine);

  void UseGrenade(Position<float> const& position);

  void Move(Position<float> const& position);

  void Stop();

  void Attack(Position<float> const& position);

  void ChooseOrigin(Position<float> const& position);

 private:
  void Loop();
  void OnMessage(Message const& message);

  hv::EventLoopPtr event_loop_;
  hv::TimerID loop_timer_id_;
  std::unique_ptr<hv::WebSocketClient> ws_client_;

  std::optional<std::vector<PlayerInfo>> all_player_info_;
  std::optional<Map> map_;
  std::optional<std::vector<Supply>> supplies_;
  std::optional<SafeZone> safe_zone_;
  std::optional<std::vector<GrenadeInfo>> grenade_info_;
  std::optional<int> self_id_;
  std::optional<int> ticks_;
  std::string token_;
};

auto format_as(Agent const& object) -> std::string;

}  // namespace thuai7_agent

#endif  // THUAI7_AGENT_AGENT_H_
