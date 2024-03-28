#include <boost/throw_exception.hpp>
#include <spdlog/spdlog.h>

namespace boost {

void throw_exception(std::exception const &e) { // NOLINT
  throw e;
}

void throw_exception(std::exception const &e, // NOLINT
                     boost::source_location const & /*loc*/) {
  throw e;
}

} // namespace boost
