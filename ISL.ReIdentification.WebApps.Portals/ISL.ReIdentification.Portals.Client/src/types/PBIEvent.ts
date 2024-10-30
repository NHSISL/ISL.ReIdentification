type Identity = {
    target: {
        column: string,
        table: string,
    },
    equals: number
}

export type PBIEvent = {
    dataPoints : Array<{
        identity : Array<Identity>
    }>
}