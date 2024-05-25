#ifndef PATH_FINDING_H_
#define PATH_FINDING_H_

#include <vector>

#include "agent/map.h"
#include "agent/position.h"

auto FindPathBeFS(thuai7_agent::Map const& map,
                  thuai7_agent::Position<int> const& start,
                  thuai7_agent::Position<int> const& end)
    -> std::vector<thuai7_agent::Position<int>>;

#endif  // PATH_FINDING_H_
