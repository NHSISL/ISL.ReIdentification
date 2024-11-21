import React from 'react'
import ReactDOM from 'react-dom/client'
import './index.css'
import '../node_modules/bootstrap/dist/css/bootstrap.min.css'
import App from './App';
import { EventType, PublicClientApplication, EventMessage, AuthenticationResult } from '@azure/msal-browser';
import { msalConfig } from './authConfig';
import { ReactPlugin } from '@microsoft/applicationinsights-react-js';
import { ApplicationInsights } from '@microsoft/applicationinsights-web';

const reactPlugin = new ReactPlugin();
const appInsights = new ApplicationInsights({
    config: {
        connectionString: import.meta.env.VITE_REACT_APP_AI_CONNECTIONSTRING,
        extensions: [reactPlugin],
        enableAutoRouteTracking: true,
    }
});
appInsights.loadAppInsights();
/**
 * MSAL should be instantiated outside of the component tree to prevent it from being re-instantiated on re-renders.
 * For more, visit: https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-react/docs/getting-started.md
 */
const msalInstance = new PublicClientApplication(msalConfig);

msalInstance.initialize().then(() => {
    // Account selection logic is app dependent. Adjust as needed for different use cases.
    const accounts = msalInstance.getAllAccounts();
    if (accounts.length > 0) {
        msalInstance.setActiveAccount(accounts[0]);
    } else {
        const silentRequest = {
            scopes: ["User.Read", "Mail.Read"],
            loginHint: "david.cunliffe2@nhs.net"
        };

       // msalInstance.ssoSilent(silentRequest);
    }

    msalInstance.addEventCallback((event: EventMessage) => {
        if (event.eventType === EventType.LOGIN_SUCCESS && event.payload) {
            const payload = event.payload as AuthenticationResult;
            const account = payload.account;
            msalInstance.setActiveAccount(account);
        }
    });

    const root = ReactDOM.createRoot(
        document.getElementById("root") as HTMLElement
    );
    
    
    root.render(
        <React.StrictMode>
            <App instance={msalInstance} />
        </React.StrictMode>
    );
});
