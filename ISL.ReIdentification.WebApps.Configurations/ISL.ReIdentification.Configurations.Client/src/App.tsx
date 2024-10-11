/* eslint-disable @typescript-eslint/no-explicit-any */
import { createBrowserRouter, Navigate, RouterProvider } from 'react-router-dom';
import './App.css';
import Root from './components/root';
import ErrorPage from './errors/error';
import { Page1 } from './pages/page1';
import { Page2 } from './pages/page2';
import { MsalProvider } from '@azure/msal-react';
import { SecuredRoute } from './components/securitys/securedRoutes';
import securityPoints from './securityMatrix';
import { Page3 } from './pages/page3';
import { QueryClientProvider } from '@tanstack/react-query';
import { queryClientGlobalOptions } from './brokers/apiBroker.globals';
import { Page4 } from './pages/page4';
import { Page5 } from './pages/page5';
import { Home } from './pages/home';
import { ConfigurationHome } from './pages/configuration/configurationHome';
import { Lookups } from './pages/configuration/lookups';
import { UserAccess } from './pages/userAccess';
import { DelegatedUserAccess } from './pages/delegatedUserAccess';
import { OdsData } from './pages/odsData';
import { PdsData } from './pages/pdsData';
import UserAccessDetail from './components/userAccess/userAccessDetail';

// TODO:
//      - API Secured Routes

function App({ instance }: any) {

    const router = createBrowserRouter([
        {
            path: "/",
            element: <Root />,
            errorElement: <ErrorPage />,
            children: [
                {
                    path: "home",
                    element: <Home />
                },
                {
                    path: "configuration/home",
                    element: <ConfigurationHome />
                },
                {
                    path: "configuration/lookups",
                    element: <Lookups />
                },
                {
                    path: "userAccess",
                    element: <UserAccess />
                },
                {
                    path: "userAccessDetail/:userDetailId",
                    element: <UserAccessDetail />
                },
                {
                    path: "delegatedUserAccess",
                    element: <DelegatedUserAccess />
                },
                {
                    path: "odsData",
                    element: <OdsData />
                },
                {
                    path: "pdsData",
                    element: <PdsData />
                },
                {
                    path: "page1/:id",
                    element: <Page1 />
                },
                {
                    path: "page2",
                    element: <SecuredRoute allowedRoles={securityPoints.page2.view}><Page2 /></SecuredRoute>
                },
                {
                    path: "page3",
                    element: <Page3 />
                },
                {
                    path: "page4",
                    element: <Page4 />
                },
                {
                    path: "page5",
                    element: <Page5 />
                },
                {
                    index: true,
                    element: <Navigate to="/home" />
                }
            ]
        }
    ]);

    return (
        <>
            <MsalProvider instance={instance}>
                <QueryClientProvider client={queryClientGlobalOptions}>
                    <RouterProvider router={router} />
                </QueryClientProvider>
            </MsalProvider>
        </>
    );


}

export default App;