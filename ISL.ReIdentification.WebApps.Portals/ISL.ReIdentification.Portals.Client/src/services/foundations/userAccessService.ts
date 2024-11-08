import { useQuery } from '@tanstack/react-query';
import UserAccessBroker from '../../brokers/apiBroker.userAccess';

export const userAccessService = {
    useSearchUserAccess: (searchTerm: string) => {
        const broker = new UserAccessBroker();

        return useQuery({
            queryKey: ["UserAccessSearch", { query: searchTerm }],
            queryFn: async () => await broker.FilterUserAccessAsync(searchTerm),
            staleTime: Infinity
        });
    }
}