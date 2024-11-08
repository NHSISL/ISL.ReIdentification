/* eslint-disable @typescript-eslint/no-explicit-any */
import { createBrowserRouter, Navigate, RouterProvider } from 'react-router-dom';
import './App.css';
import Root from './components/root';
import ErrorPage from './errors/error';
import { MsalProvider } from '@azure/msal-react';
import { SecuredRoute } from './components/securitys/securedRoutes';
import securityPoints from './securityMatrix';
import { QueryClientProvider } from '@tanstack/react-query';
import { queryClientGlobalOptions } from './brokers/apiBroker.globals';
import { Home } from './pages/home';
import { ReIdentificationPage } from './pages/reIdentification';
import { CsvReIdentificationPage } from './pages/csvReIdentification';
import { ReIdReportHome } from './pages/reidReportHome';
import ReportsHome from './components/reports/reportsHome';
import { WebPart } from './pages/webPart';
import { CsvReIdentificationDownloadPage } from './pages/csvReIdentificationDownload';
import { CsvReIdentificationWorklistPage } from './pages/csvReIdentificationWorklist';
import { ImpersonationProjectAddPage } from './pages/impersonationProjectAddPage';
import { ImpersonationProjectPage } from './pages/impersonationProjectPage';
import { ImpersonationManagePage } from './pages/impersonationManagePage';

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
                    path: "reIdentification",
                    element: <SecuredRoute allowedRoles={securityPoints.reIdentification.view}><ReIdentificationPage /></SecuredRoute>
                },
                {
                    path: "csvReIdentification",
                    element: <CsvReIdentificationPage />
                },
                {
                    path: "report",
                    element: <ReIdReportHome />
                },
                {
                    path: "csvReIdentificationWorklist",
                    element: <CsvReIdentificationWorklistPage />
},
                {
                    path: "csvReIdentification/:csvIdentificationRequestId",
                    element: <CsvReIdentificationDownloadPage />
                },
                {
                    path: "project",
                    element: <ImpersonationProjectPage />
                },
                {
                    path: "addProject",
                    element: <ImpersonationProjectAddPage />
                },
                {
                    path: "project/:impersonationIdentificationRequestId",
                    element: <ImpersonationManagePage />
                },
                {
                    path: "approveProject",
                    element: <ImpersonationProjectAddPage />
                },
                {
                    index: true,
                    element: <Navigate to="/home" />
                }
            ]
        },
        {
            path: "report/:reportGroupId/:reportId/:pseudoColumn/:reportPage?",
            element: <ReportsHome />
        },
        {
            path: "webpart/:pseudoId",
            element: <WebPart />
        },  
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