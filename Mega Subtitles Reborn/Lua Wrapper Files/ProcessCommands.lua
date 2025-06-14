local ProcessCommands = {}

-- Cache required modules
local ParseDeleteCreate = require("ParseDeleteCreate")
local SyncWithCSharp = require("SyncWithCSharp")
local SyncCursorPosition = require("SyncCursorPosition")
local ParseCacheFile = require("ParseCacheFile")
local CheckForMissing = require("CheckForMissing")
local CheckForRepeats = require("CheckForRepeats")
local SetGlobalVariables = require("SetGlobalVariables")

local actors = ""

-- Command lookup table for better performance and cleaner code
local COMMAND_HANDLERS = {
    CreateRegionsWithOutColor = function() 
        ParseDeleteCreate.regions("WithOutColor", actors) -- Create regions without color
    end,
    
    CreateRegionsWithColor = function() 
        ParseDeleteCreate.regions("WithColor", actors) -- Create regions with color
    end,
    
    UnColorizeAllTracks = function() 
        ParseDeleteCreate.tracks("WithOutColor", actors) -- Uncolorize all tracks
    end,
    
    ColorizeAllTracks = function() 
        ParseDeleteCreate.tracks("WithColor", actors) -- Colorize all tracks
    end,
    
    ColorizeSelectedTracks = function() 
        ParseDeleteCreate.tracks("WithColor", actors) -- Colorize selected tracks
    end,
    
    ColorizeSelectedActors = function() 
        ParseDeleteCreate.regions("WithColor", actors) -- Colorize selected actors
    end,
    
    ColorizeSelectedActorsComments = function() 
        ParseDeleteCreate.regions("WithComments", actors) -- Colorize selected actors with comments
    end,
    
    RegionsOnlyWithComments = function() 
        ParseDeleteCreate.regions("OnlyWithComments", actors) -- Create regions only with comments
    end,
    
    CheckForMissing = function()        
        CheckForMissing.checkingAndColoring() -- Check for missing replics and color them
    end,
        
    CheckForRepeats = function()
        CheckForRepeats.check() -- Check for repeated replics
    end,

    FindDemoPhrases = function() 
        FindDemoPhrases.find() -- Find demo phrases in the project
    end,

    SyncPositon = function() 
        SyncWithCSharp.Sync() -- Sync the position with C# application
    end,
    
    SyncCursorPosition = function() 
        SyncCursorPosition.Sync() -- Sync the position with C# application
    end,

    SyncProject = function() 
        SetGlobalVariables.set() -- Set global variables for the project
    end,    
    
    ApplicationCloseEvent = function()     
	    gfx.quit() -- Close the gfx window
    end
}

function ProcessCommands.Process(command, selectedActors) -- Process the command received from C# application
    actors = selectedActors
    local handler = COMMAND_HANDLERS[command] -- Lookup the command handler in the table
    if handler then -- If a handler exists for the command
        handler() -- Call the handler with the provided CacheFilePath
    end
end

return ProcessCommands
