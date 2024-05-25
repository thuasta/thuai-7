#ifndef THUAI7_AGENT_POSITION_H_
#define THUAI7_AGENT_POSITION_H_

#include <fmt/format.h>

#include <string>
#include <type_traits>

namespace thuai7_agent {

template <typename T>
  requires std::is_arithmetic_v<T>
struct Position {
  T x;
  T y;
};

template <typename T>
  requires std::is_arithmetic_v<T>
auto operator==(Position<T> const& lhs, Position<T> const& rhs) -> bool {
  return lhs.x == rhs.x && lhs.y == rhs.y;
}

template <typename T>
  requires std::is_arithmetic_v<T>
auto format_as(Position<T> const& object) -> std::string {
  return fmt::format("Position{{x: {}, y: {}}}", object.x, object.y);
}

}  // namespace thuai7_agent

#endif  // THUAI7_AGENT_POSITION_H_
