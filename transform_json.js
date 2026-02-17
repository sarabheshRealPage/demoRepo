const fs = require('fs');

// Read the source JSON file
const sourceFile = 'C:\\Users\\skankatala\\OneDrive - RealPage\\Dell_laptop_2_drive\\DataHub\\TC_LIHTC\\30_percentage_already_exists.json';
const outputFile = 'C:\\Users\\skankatala\\OneDrive - RealPage\\Dell_laptop_2_drive\\DataHub\\TC_LIHTC\\30_percentage_transformed.json';

console.log('Reading source file...');
const jsonContent = JSON.parse(fs.readFileSync(sourceFile, 'utf8'));

// Extract the value object from the first record (Kafka message wrapper)
const valueObject = jsonContent[0].value;

console.log('Transforming data structure...');

// Function to recursively convert Avro union types to simple values
function convertAvroToSimple(obj) {
    if (obj === null || obj === undefined) {
        return obj;
    }
    
    if (typeof obj !== 'object') {
        return obj;
    }
    
    // Check if this is an Avro union type with 'string' property
    if (obj.hasOwnProperty('string')) {
        return obj.string;
    }
    
    // Handle arrays
    if (Array.isArray(obj)) {
        return obj.map(item => convertAvroToSimple(item));
    }
    
    // Handle objects - recursively process all properties
    const newObj = {};
    for (const key in obj) {
        if (obj.hasOwnProperty(key)) {
            newObj[key] = convertAvroToSimple(obj[key]);
        }
    }
    return newObj;
}

// Convert the value object to match the reference format
const transformedObject = convertAvroToSimple(valueObject);

console.log('Writing output file...');
fs.writeFileSync(outputFile, JSON.stringify(transformedObject, null, 2), 'utf8');

console.log('Transformation complete! Output saved to:', outputFile);
const stats = fs.statSync(outputFile);
console.log('File size:', stats.size, 'bytes');
