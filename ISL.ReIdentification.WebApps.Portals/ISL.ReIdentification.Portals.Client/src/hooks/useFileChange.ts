export const useFileChange = (setError: React.Dispatch<React.SetStateAction<string[]>>, setFileName: React.Dispatch<React.SetStateAction<string>>, setHeaderColumns: React.Dispatch<React.SetStateAction<string[]>>, setCsvData: React.Dispatch<React.SetStateAction<string | null>>, hasHeaderRecord: boolean) => {
    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setError([]);
        const file = e.target.files?.[0];
        if (file) {
            const sizeInBytes = file.size;

            if (sizeInBytes > 1024 * 1024) {
                setError(["File size exceeds 1MB. Please upload a smaller file."]);
                return;
            }

            if (file.name.endsWith(".csv")) {
                setFileName(file.name);
                const reader = new FileReader();
                reader.onload = (event) => {
                    let text = event.target?.result as string;
                    text = text.replace(/\r/g, "");
                    const rows = text.split("\n");
                    setHeaderColumns([]);

                    if (hasHeaderRecord) {
                        const headers = rows.length === 1 ? [rows[0]] : rows[0].split(",");

                        if (headers.length <= 0 ||
                            (rows.length > 1 && !/[a-zA-Z]/.test(rows[0]))) {
                            setError(["The CSV file does not contain a valid header row."]);
                            return;
                        }

                        setHeaderColumns(headers);
                    } else {
                        const headers = rows.length === 1 ? [rows[1]] : rows[1].split(",");
                        setHeaderColumns(headers);
                    }

                    const uint8Array = new TextEncoder().encode(text);
                    const base64String = btoa(String.fromCharCode(...uint8Array));
                    setCsvData(base64String);
                };
                reader.readAsText(file);
            } else {
                setError(["Please upload a valid .csv file."]);
            }
        } else {
            setError(["No file selected. Please upload a .csv file."]);
        }
    };

    return { handleFileChange };
};