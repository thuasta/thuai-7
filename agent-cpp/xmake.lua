add_rules("mode.debug", "mode.release")

add_requires("cxxopts v3.2.1")
add_requires("fmt 10.2.1")
add_requires("libhv 1.3.2", {configs={http_server=false}})
add_requires("magic_enum v0.9.5")
add_requires("nlohmann_json v3.11.3")
add_requires("spdlog v1.13.0")

target("agent")
    add_files("src/**.cc")
    add_includedirs("src")
    add_packages(
        "cxxopts",
        "fmt",
        "libhv",
        "magic_enum",
        "nlohmann_json",
        "spdlog"
    )
    set_exceptions("cxx")
    set_kind("binary")
    set_languages("cxx20")
    set_warnings("allextra")

    if is_plat("windows") then
        add_defines("NOMINMAX")
    end

    after_build(function (target)
        os.cp(
            target:targetfile(), 
            path.join(os.projectdir(), "bin", path.filename(target:targetfile()))
        )
    end)
