add_rules("mode.debug", "mode.release")

add_requires("boost 1.84.0")
add_requires("spdlog v1.13.0")
add_requires("websocketpp 0.8.2")

target("bot")
    add_files("src/**.cpp")
    add_headerfiles("src/(**.h)")
    add_includedirs("src")
    add_packages(
        "boost",
        "spdlog",
        "websocketpp"
    )
    set_exceptions("cxx")
    set_kind("binary")
    set_languages("cxx17")
