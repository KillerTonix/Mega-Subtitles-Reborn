------------------------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------------------------
--  __  __                     _____       _     _   _ _   _            __          __                                --
-- |  \/  |                   / ____|     | |   | | (_) | | |           \ \        / /                                --
-- | \  / | ___  __ _  __ _  | (___  _   _| |__ | |_ _| |_| | ___  ___   \ \  /\  / / __ __ _ _ __  _ __   ___ _ __   --
-- | |\/| |/ _ \/ _` |/ _` |  \___ \| | | | '_ \| __| | __| |/ _ \/ __|   \ \/  \/ / '__/ _` | '_ \| '_ \ / _ \ '__|  --
-- | |  | |  __/ (_| | (_| |  ____) | |_| | |_) | |_| | |_| |  __/\__ \    \  /\  /| | | (_| | |_) | |_) |  __/ |     --
-- |_|  |_|\___|\__, |\__,_| |_____/ \__,_|_.__/ \__|_|\__|_|\___||___/     \/  \/ |_|  \__,_| .__/| .__/ \___|_|     --
--               __/ |                                                                       | |   | |                --
--              |___/                                                                        |_|   |_|                --
------------------------------------------------------------------------------------------------------------------------	
------------------------------------------------------------------------------------------------------------------------	
-- Wrapper for Mega Subtitles app 																					  --
-- By Artur Tonoyan                             																	  --
-- Ver: v0.0.1		                                                                                                  --
-- Dependencies: json.lua, utf8_filenames.lua                                                                         --
-- VK: vk.com/aatonoyan                                                                                               --
-- Discord: arturtonoyan                                                                                              --
-- Email: Artur.tonoyan2012@yandex.ru                                                                                 --  
-- GitHub: github.com/ArturTonoyan/Mega-Subtitles-Reborn                                                              --
-- This script is a wrapper for Mega Subtitles Reborn application, allowing it to communicate with REAPER via Lua.    --
-- It reads commands from a file, processes them, and interacts with the application.                                 --
------------------------------------------------------------------------------------------------------------------------		  
------------------------------------------------------------------------------------------------------------------------

-- <Set up paths> --
script_path = debug.getinfo(1, "S").source:match("@(.*[/\\])") -- Get the script path
package.path = package.path .. ";" .. script_path .. "Lua Wrapper Files/?.lua" -- Add the Lua Wrapper Files directory to the package path
-- </Set up paths/> --

-- <Get Windows Codepage> --
local function get_windows_codepage() -- Get the Windows codepage
    local pipe = assert(io.popen[[reg query HKLM\SYSTEM\CurrentControlSet\Control\Nls\CodePage /v ACP]]) -- Open a pipe to the Windows registry to read the codepage
    local codepage = pipe:read"*a":match"%sACP%s+REG_SZ%s+(.-)%s*$" -- Read the output and extract the codepage value
    pipe:close() -- Close the pipe
    return codepage  -- returns string like "1251"
 end
-- </Get Windows Codepage/> --

-- <Convert script path to ANSI if codepage is 1251> --
if get_windows_codepage() == "1251" then -- Check if the codepage is 1251 (Cyrillic)
    local convert_from_utf8 = require("utf8_filenames") -- Import the utf8_filenames module to convert UTF-8 paths to ANSI
    script_path = convert_from_utf8(script_path) -- Convert the script path from UTF-8 to ANSI
    package.path = package.path .. ";" .. script_path .. "Lua Wrapper Files/?.lua" -- Update the package path with the converted script path
end
-- </Convert script path to ANSI if codepage is 1251/> --

-- <DirectoryExists/> --
local function directoryExists(path) -- Check if a directory exists
	local ok, err, code = os.rename(path, path) -- Try to rename the directory to itself
	if not ok and code == 13 then -- Permission denied, but directory exists
		return true
	end
	return ok
end
-- </DirectoryExists/> --


-- <Require List> --
local ReadCommandFromCSharp = require("ReadCommandFromCSharp") -- Read commands from the C# application
local ClearCommandsFile = require("ClearCommandsFile") -- Clear the commands file before starting
local GetCurrentProjectName = require("GetCurrentProjectName") -- Get the current project name
local ProcessCommands = require("ProcessCommands") -- Process commands received from the C# application
-- </Require List/> --


-- <Global Variables>--
CacheFolderPath = script_path .. "Cache" -- Path to the cache folder
ExecutableCommandsFilePath = CacheFolderPath .. "\\ReaperCommands.json" -- Path to the commands file
ExecutableMegaSubtitlesPath = '"' .. script_path .. "Mega Subtitles Reborn.exe" .. '"' -- Path to the Mega Subtitles Reborn executable
CurrentProjectNameFilePath = CacheFolderPath .. "\\CurrentProjectName.txt" -- Path to the current project name file
SyncForCSharpFilePath = CacheFolderPath .. "\\SyncForCSharp.txt" -- Path to the sync file for C# application

CurrentProjectName = "" -- Variable to store the current project name
CacheFilePath = "" -- Variable to store the cache file path
CursorSync = false -- Flag to indicate whether to sync the cursor position with C# application
CurrentPosition = "" -- Variable to store the current position in the project
ProjectID = "" -- Variable to store the current project ID
NumerOfTracks = 0 -- Variable to store the number of tracks in the current project
CountOfProjectMarkers = 0 -- Variable to store the count of project markers in the current project
-- </Global Variables/>--


--<Main Function> --
function Main()
	gfx.init("Mega Subtitles", 10, 10, 0, 1899, 1010) -- Draw Window	
	if not directoryExists('"' .. CacheFolderPath.. '"') then -- Check if ApplicationPath exists
		os.execute("mkdir " .. '"' .. CacheFolderPath.. '"'	)	-- Create ApplicationPath if !Exists
	end

    CurrentProjectName = GetCurrentProjectName.getName() -- Get the current project name and save it to a file
	ClearCommandsFile.Clear() -- Clear commands file
    ProjectID = reaper.EnumProjects(-1, "") -- Get the current project ID
    NumerOfTracks = reaper.CountTracks(ProjectID) -- Get the number of tracks in the current project
    _, _, CountOfProjectMarkers = reaper.CountProjectMarkers(ProjectID) -- Get the count of project markers in the current project

	os.execute("START" .. ' "" ' ..  ExecutableMegaSubtitlesPath) -- Run Main Application
    	
	local lastDateAndTIme = nil
    
    --<Loop Function> --
    local function loop()            
        local command, cacheFilePath, currentPosition, commandDateAndTime = ReadCommandFromCSharp.ReadCommand() -- Read command from C# application
        CacheFilePath = cacheFilePath -- Update the cache file path
        CurrentPosition = currentPosition -- Update the current position in the project
        if commandDateAndTime ~= lastDateAndTIme then -- Check if the command date and time is different from the last processed command
            lastDateAndTIme = commandDateAndTime -- Update the last processed command date and time
            if command then -- If a command was read
                ProcessCommands.Process(command) -- Process the command
            end
        end
       
        if gfx.getchar() >= 0 then
            reaper.defer(loop)
        else                                                              
            os.execute("TASKKILL /IM 'Mega Subtitles Reborn.exe'")
            os.execute("TASKKILL /IM 'Mega Subtitles Reborn.exe'")
            os.execute("TASKKILL /IM 'Mega Subtitles Reborn.exe'")			
            gfx.quit()            
        end
    end
	--</Loop Function/> --
	
	
	loop() -- Call Loop Function	
end
--</Main Function/> --


Main() -- Call Main Function