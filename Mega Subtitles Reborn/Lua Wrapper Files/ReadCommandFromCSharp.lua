local ReadCommandFromCSharp = {}

function ReadCommandFromCSharp.ReadCommand()
	local Json = require("json") -- Ensure json.lua is available in the package path
	local file = io.open(ExecutableCommandsFilePath, "r") -- Open the file for reading
	local CommandsFile = file:read( "*all" ) -- Read the entire content of the file
	file:close() -- Close the file after reading

	if string.len (CommandsFile) > 0 then -- Check if the file was opened successfully		
		local DecodedJson = Json.decode(CommandsFile) -- Decode the JSON content
		local Command = DecodedJson["Command"] 	-- Extract the command from the decoded JSON
		local CachePath = DecodedJson["CachePath"] -- Extract the argument from the decoded JSON
		local CurrentPosition = DecodedJson["CurrentPosition"] -- Extract the date and time from the decoded JSON	
		local DateAndTime = DecodedJson["DateAndTime"] -- Extract the date and time from the decoded JSON	

		if Command and CachePath and DateAndTime and CurrentPosition then -- Check if both command and argument are present
			return Command, CachePath, CurrentPosition, DateAndTime -- Return the command and argument, removing any newline characters
		end
	end	
	
end

return ReadCommandFromCSharp
