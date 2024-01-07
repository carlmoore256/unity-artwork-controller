import { IEndpoint } from "./unityEndpoints"
export const UNITY_ENDPOINTS: IEndpoint = {
    id: "base",
    directory: "unity",
    label: "Base",
    type: null,
    children: [
        {
            id: "1",
            directory: "artwork",
            label: "Artwork",
            type: null,
            children: [
                {
                    id: "1.1",
                    directory: "motion",
                    label: "motion",
                    type: null,
                    children: [
                        {
                            id: "1.1.1",
                            directory: "sinX",
                            label: "sinX",
                            type: "float",
                            children: []
                        }
                    ]
                }
            ]
        },
        {
            id: "2",
            directory: "artwork",
            label: "Artwork",
            type: null,
            children: [
                {
                    id: "2.1",
                    directory: "motifs",
                    label: "Motifs",
                    type: null,
                    children: [
                        {
                            id: "2.1.1",
                            directory: ":motifIdx",
                            label: "Index",
                            type: "variable",
                            children: [
                                {
                                    id: "2.1.1.1",
                                    directory: "color",
                                    label: "Color",
                                    type: null,
                                    children: [
                                        {
                                            id: "2.1.1.1.1",
                                            directory: "shift",
                                            label: "shift",
                                            type: "float",
                                            children: []
                                        }
                                    ]
                                }
                            ]
                        }
                    ]
                }
            ]
        }
    ]
}