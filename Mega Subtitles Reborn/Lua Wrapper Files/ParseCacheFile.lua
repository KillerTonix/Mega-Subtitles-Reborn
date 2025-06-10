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

-- Command type handlers
local COMMAND_HANDLERS = {
    WithColor = function(color) 
        return colorToNative(color)
    end,
    
    WithComments = function(comment)
        if comment and not find(comment, NO_COMMENT) then
            return ColorToNative(255, 0, 0) | 0x1000000
        end
        return "0"
    end,
    
    OnlyWithComments = function()
        return "0"
    end,
    
    WithOutColor = function()
        return "0"
    end
}

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

function isColored(commandType, color, comment)
    if commandType == "WithColor" then
        return colorToNative(color)    
    elseif commandType == "WithComments" then
        if #comment > 0 then
            return ColorToNative(255, 0, 0) | 0x1000000
        else
            return 0 -- No color for empty comments
        end
    else 
        return 0 -- No color for other command types
    end    
end


function ParseCacheFile.parse(commandType)
    local content = load_json_file() -- Load the JSON content from the file
    local data = json.decode(content) -- Decode the JSON content
    local handler = COMMAND_HANDLERS[commandType] -- Get the handler for the command type
    local subtitleEntries = {} -- Initialize the table to hold subtitle entries
    local start_time, end_time, text ,color, actor, comment = "", "", "", "", "", "" 

    for i, entry in ipairs(data.Entries) do -- Iterate through each entry in the data
        start_time = entry.Start or "" 
        end_time = entry.End or ""
        text = entry.Text or ""        
        actor = entry.Actor or ""
        comment = entry.Comment or ""
        color = isColored(commandType, entry.Color, comment)

        if start_time and end_time and text and color and actor and comment then
            if commandType == "OnlyWithComments" and comment == "" then 
                goto continue
            end

            local entry = {
                Start = timeToSeconds(start_time),
                End = timeToSeconds(end_time),
                Text = text,
                Color = color, -- Use the handler to process color or comment
                Actor = actor,
                Comment = comment
            }

            if not subtitleEntries[entry.Actor] then -- Check if the actor already exists in the table
                subtitleEntries[entry.Actor] = {} -- If not, create a new table for the actor
            end

            table.insert(subtitleEntries[entry.Actor], entry) -- Insert the entry into the actor's table
        end
        ::continue:: -- Continue to the next iteration if the entry is not valid
    end
    
    return subtitleEntries
end

return ParseCacheFile
