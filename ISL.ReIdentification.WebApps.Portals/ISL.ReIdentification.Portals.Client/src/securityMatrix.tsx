const securityPoints = {
    configuration: {
        add: ['ISL.Reidentification.Configuration.Administrators', 'ISL.Reidentification.Configuration.Users'],
        edit: ['ISL.Reidentification.Configuration.Administrators', 'ISL.Reidentification.Configuration.Users'],
        delete: ['ISL.Reidentification.Configuration.Administrators', 'ISL.Reidentification.Configuration.Users'],
        view: ['ISL.Reidentification.Configuration.Administrators', 'ISL.Reidentification.Configuration.Users'],
    },
    lookups: {
        add: ['ISL.Reidentification.Configuration.Administrators', 'ISL.Reidentification.Configuration.Users'],
        edit: ['ISL.Reidentification.Configuration.Administrators', 'ISL.Reidentification.Configuration.Users'],
        delete: ['ISL.Reidentification.Configuration.Administrators', 'ISL.Reidentification.Configuration.Users'],
        view: ['ISL.Reidentification.Configuration.Administrators', 'ISL.Reidentification.Configuration.Users'],
    },
    userAccess: {
        add: ['ISL.Reidentification.Configuration.Administrators', 'ISL.Reidentification.Configuration.Users'],
        edit: ['ISL.Reidentification.Configuration.Administrators', 'ISL.Reidentification.Configuration.Users'],
        delete: ['ISL.Reidentification.Configuration.Administrators', 'ISL.Reidentification.Configuration.Users'],
        view: ['ISL.Reidentification.Configuration.Administrators', 'ISL.Reidentification.Configuration.Users'],
    },
    singleReId: {
        add: [],
        edit: [],
        delete: [],
        view: [],
    },
    csvReId: {
        add: [],
        edit: [],
        delete: [],
        view: [],
    },
    csvWorklist: {
        add: [],
        edit: [],
        delete: [],
        view: [],
    },
    reports: {
        add: [],
        edit: [],
        delete: [],
        view: [],
    },
    impersonation: {
        add: ['ISL.Reidentification.Configuration.Administrators', 'ISL.Reidentification.Portal.DataEngineers'],
        edit: ['ISL.Reidentification.Configuration.Administrators', 'ISL.Reidentification.Portal.DataEngineers'],
        delete: ['ISL.Reidentification.Configuration.Administrators', 'ISL.Reidentification.Portal.DataEngineers'],
        view: ['ISL.Reidentification.Configuration.Administrators', 'ISL.Reidentification.Portal.DataEngineers'],
    }
}

export default securityPoints