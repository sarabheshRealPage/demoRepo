# Read the source JSON file (Kafka message format)
$sourceFile = "C:\Users\skankatala\OneDrive - RealPage\Dell_laptop_2_drive\DataHub\TC_LIHTC\dec_12_2025_Null_error.json"
$outputFile = "C:\Users\skankatala\OneDrive - RealPage\Dell_laptop_2_drive\DataHub\TC_LIHTC\transformed_payload.json"

Write-Host "Reading source file..."
$jsonContent = Get-Content $sourceFile -Raw | ConvertFrom-Json

# Extract the value object from the first record (Kafka message wrapper)
$valueObject = $jsonContent[0].value

Write-Host "Transforming data structure..."

# Function to recursively convert objects with 'string' property to simple values
function Convert-AvroToSimple {
    param($obj)
    
    if ($null -eq $obj) {
        return $null
    }
    
    if ($obj -is [System.Management.Automation.PSCustomObject]) {
        # Check if this object has a 'string' property (Avro union type)
        if ($obj.PSObject.Properties.Name -contains 'string') {
            return $obj.string
        }
        
        # Otherwise, recursively process all properties
        $newObj = [PSCustomObject]@{}
        foreach ($prop in $obj.PSObject.Properties) {
            $newObj | Add-Member -MemberType NoteProperty -Name $prop.Name -Value (Convert-AvroToSimple $prop.Value)
        }
        return $newObj
    }
    elseif ($obj -is [System.Array]) {
        # Process each array element
        return @($obj | ForEach-Object { Convert-AvroToSimple $_ })
    }
    else {
        # Return primitive values as-is
        return $obj
    }
}

# Convert the value object to match the reference format
$transformedObject = Convert-AvroToSimple $valueObject

Write-Host "Writing output file..."
$transformedObject | ConvertTo-Json -Depth 100 | Set-Content $outputFile -Encoding UTF8

Write-Host "Transformation complete! Output saved to: $outputFile"
Write-Host "File size: $((Get-Item $outputFile).Length) bytes"
