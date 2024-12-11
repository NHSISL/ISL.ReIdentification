import { useEffect, useState } from "react";
import FrontendConfigurationBroker, { FrontendConfiguration } from "../brokers/apiBroker.frontendConfigurationBroker";
import { useQuery } from "@tanstack/react-query";

export function useFrontendConfiguration() { 
    const [configuration, setConfiguration] = useState<FrontendConfiguration>();

    const broker = new FrontendConfigurationBroker();

    const query = useQuery({
        queryKey: ["FrontendConfiguration"],
        queryFn: async () => await broker.GetFrontendConfigruationAsync(),
        staleTime: Infinity
    });

    useEffect(() => {
        setConfiguration(query.data);
    },[query.data]);
        
    return { configuration }
}