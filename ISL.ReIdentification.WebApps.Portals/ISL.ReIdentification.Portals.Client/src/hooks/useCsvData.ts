export const useCsvData = (csvData: string | null, headerColumns: string[], setError: React.Dispatch<React.SetStateAction<string[]>>, setSuccess: React.Dispatch<React.SetStateAction<string>>) => {
    const handleHeaderColumnChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const selectedColumn = e.target.value;
        const index = headerColumns.indexOf(selectedColumn);

        if (csvData) {
            const decodedData = atob(csvData);
            const rows = decodedData.split("\n");

            if (rows.length > 1) {
                const nextRow = rows[1].split(",");
                const value = nextRow[index];

                if (/^\d{10}$/.test(value)) {
                    setError([]);
                    setSuccess(`The value "${value}" in the next row at the selected column index is 10 digits long and is a valid Pseudo Identifier.`);
                } else {
                    setSuccess("");
                    setError([`The value "${value}" in the next row for the selected column index is not a valid Pseudo Identifier, please follow the guidance in the help section.`]);
                }
            }
        }
    };

    return { handleHeaderColumnChange };
};