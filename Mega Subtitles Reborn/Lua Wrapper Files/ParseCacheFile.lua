-- ParseCacheFile.lua
local ParseCacheFile = {}

local json = require("json") -- assuming json.lua is in the same directory
local color = require "colorise"

-- Cache frequently used functions
local match = string.match
local ColorToNative = reaper.ColorToNative
local find = string.find

-- Pre-compile patterns
local TIME_PATTERN_LONG = "(%d+):(%d+):(%d+).(%d+)"
local TIME_PATTERN_SHORT = "(%d+):(%d+).(%d+)"
local COLOR_PATTERN = "(%d+),(%d+),(%d+)"
local TRIM_PATTERN = "^%s*(.-)%s*$"

-- Function to trim whitespace from a string
local function trim(s)
    return (s:gsub(TRIM_PATTERN, "%1"))
end

-- Function to convert time string to seconds
local function timeToSeconds(timeStr)
    local hours, minutes, seconds, milliseconds = 0,0,0,0
    
    if #timeStr > 9 then
        hours, minutes, seconds, milliseconds = match(timeStr, TIME_PATTERN_LONG)
    else 
        hours = 0
        minutes, seconds, milliseconds = match(timeStr, TIME_PATTERN_SHORT)
    end
    
    local milliseconds_divider = #milliseconds >= 2 and 1000 or 100
    
    return tonumber(hours) * 3600 + 
           tonumber(minutes) * 60 + 
           tonumber(seconds) + 
           (tonumber(milliseconds) / milliseconds_divider)
end

-- Function to convert color hex to native color value
local function colorToNative(hex)
    local a,r,g,b = color.hex2rgba(hex)
    return ColorToNative(r, g, b) | 0x1000000
end

-- Load the JSON content from a file
local function load_json_file()
    local file = io.open(CacheFilePath, "r") -- Open the file for reading
    if not file then 
        error("Could not open file: " .. CacheFilePath)
    end
    local content = file:read("*a") -- Read the entire file content
    file:close() -- Close the file after reading
    return content
end

function isColored(commandType, color, comment, actors, entryActor)
    if commandType == "WithColor" then
        if contains(actors, entryActor) then
            return colorToNative(color)
        else
            return 0
        end
    elseif commandType == "WithComments" then
        if #comment > 0 then
            return ColorToNative(255, 0, 0) | 0x1000000
        else
            return 0
        end
    else 
        return 0
    end    
end


function ParseCacheFile.parse(commandType, actors)
    local content = load_json_file() -- Load the JSON content from the file
    local data = json.decode(content) -- Decode the JSON content
    local subtitleEntries = {} -- Initialize the table to hold subtitle entries
    local start_time, end_time, text ,color, actor, comment = "", "", "", "", "", "" 

    for i, entry in ipairs(data.Entries) do
        actor = entry.Actor or ""

        -- Filter by actor for WithOutColor
        if commandType == "WithOutColor" and not contains(actors, actor) then
            goto continue
        end

        start_time = entry.Start or "" 
        end_time = entry.End or ""
        text = entry.Text or ""        
        comment = entry.Comment or ""
        color = isColored(commandType, entry.Color, comment, actors, actor)

        if start_time and end_time and text and color ~= nil and actor and comment then
            if commandType == "OnlyWithComments" and comment == "" then 
                goto continue
            end

            local entry = {
                Start = timeToSeconds(start_time),
                End = timeToSeconds(end_time),
                Text = text,
                Color = color,
                Actor = actor,
                Comment = comment
            }

            if not subtitleEntries[actor] then
                subtitleEntries[actor] = {}
            end

            table.insert(subtitleEntries[actor], entry)
        end
        ::continue::
    end

    
    return subtitleEntries
end

local function contains(tbl, val)
    for _, v in ipairs(tbl) do
        if v == val then
            return true
        end
    end
    return false
end


return ParseCacheFile
