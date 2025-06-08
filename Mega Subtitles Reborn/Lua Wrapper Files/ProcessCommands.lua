local ProcessCommands = {}

-- Cache required modules
local ParseDeleteCreate = require("ParseDeleteCreate")
local SyncWithCSharp = require("SyncWithCSharp")
local SyncCsToReaper = require("SyncCsToReaper")
local ParseCacheFile = require("ParseCacheFile")
local CheckForMissing = require("CheckForMissing")
local CheckForRepeats = require("CheckForRepeats")

-- Command lookup table for better performance and cleaner code
local COMMAND_HANDLERS = {
    CreateRegionsWithOutColor = function() 
        ParseDeleteCreate.regions("WithOutColor", false) -- Create regions without color
    end,
    
    CreateRegionsWithColor = function() 
        ParseDeleteCreate.regions("WithColor", false) -- Create regions with color
    end,
    
    UnColorizeAllTracks = function() 
        ParseDeleteCreate.tracks("WithOutColor", false) -- Uncolorize all tracks
    end,
    
    ColorizeAllTracks = function() 
        ParseDeleteCreate.tracks("WithColor", false) -- Colorize all tracks
    end,
    
    ColorizeSelectedTracks = function() 
        ParseDeleteCreate.tracks("WithColor", true) -- Colorize selected tracks
    end,
    
    ColorizeSelectedActors = function() 
        ParseDeleteCreate.regions("WithColor", true) -- Colorize selected actors
    end,
    
    ColorizeSelectedActorsComments = function() 
        ParseDeleteCreate.regions("WithComments", true) -- Colorize selected actors with comments
    end,
    
    RegionsOnlyWithComments = function() 
        ParseDeleteCreate.regions("OnlyWithComments", false) -- Create regions only with comments
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
    
    SyncCsToReaper = function() 
        SyncCsToReaper.Sync() -- Sync the position with C# application
    end,
    
    ApplicationCloseEvent = function()
        os.execute("TASKKILL /IM 'Mega Subtitles Reborn.exe'") -- Kill the Mega Subtitles Reborn process
	    os.execute("TASKKILL /IM 'Mega Subtitles Reborn.exe'") -- Kill the Mega Subtitles Reborn process
	    os.execute("TASKKILL /IM 'Mega Subtitles Reborn.exe'") -- Kill the Mega Subtitles Reborn process
	    gfx.quit() -- Close the gfx window
    end
}

function ProcessCommands.Process(command) -- Process the command received from C# application
    local handler = COMMAND_HANDLERS[command] -- Lookup the command handler in the table
    if handler then -- If a handler exists for the command
        handler() -- Call the handler with the provided CacheFilePath
    end
end

return ProcessCommands
