const securityPoints = {
    configuration: {
        add: ['ISL.Reidentification.Configuration.Administrators', 'ISL.Reidentification.Configuration.Users'],
        edit: ['ISL.Reidentification.Configuration.Administrators', 'ISL.Reidentification.Configuration.Users'],
        delete: ['ISL.Reidentification.Configuration.Administrators', 'ISL.Reidentification.Configuration.Users'],
        view: ['ISL.Reidentification.Configuration.Administrators', 'ISL.Reidentification.Configuration.Users'],
    },
    lookups: {
        add: ['ISL.Reidentification.Configuration.Administrators'],
        edit: ['ISL.Reidentification.Configuration.Administrators'],
        delete: ['ISL.Reidentification.Configuration.Administrators'],
        view: ['ISL.Reidentification.Configuration.Administrators', 'ISL.Reidentification.Configuration.Users'],
    },
    userAccess: {
        add: ['ISL.Reidentification.Configuration.Administrators', 'ISL.Reidentification.Configuration.Users'],
        edit: ['ISL.Reidentification.Configuration.Administrators', 'ISL.Reidentification.Configuration.Users'],
        delete: ['ISL.Reidentification.Configuration.Administrators', 'ISL.Reidentification.Configuration.Users'],
        view: ['ISL.Reidentification.Configuration.Administrators', 'ISL.Reidentification.Configuration.Users'],
    },

}

export default securityPoints