import { useQuery } from "@tanstack/react-query";
import OdsDataBroker from "../../brokers/apiBroker.odsData";

export const odsDataService = {
    useRetrieveAllOdsData: (query: string) => {
        const broker = new OdsDataBroker();

        return useQuery({
            queryKey: ["OdsDataGetAll", { query: query }],
            queryFn: () => broker.GetAllOdsDataAsync(query),
            staleTime: Infinity
        });
    },  

    useGetOdsChildren: (id: string) => {
        const broker = new OdsDataBroker();

        return useQuery({
            queryKey: ["OdsChildren", {query: id}],
            queryFn: () => broker.GetOdsChildrenByIdAsync(id),
            staleTime: Infinity
        })
    }
}