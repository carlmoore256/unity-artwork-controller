
export type Bang = "*";

export interface ArtworkEndpoint {
    id: number;

}


export interface Motion { 
    sinX : number;
    sinY : number;
    sinZ : number;
    cosX : number;
    cosY : number;
    cosZ : number;
    range : number;
    speed : number;
    scale : number;
    origPosPercent : number;
}

export interface Transform {
    position : number[];
    rotation : number;
    scale : number[];
    reset : Bang;
}

export interface Color {
    fadeIn : Bang;
    fadeOut : Bang;
    opacity : number;
    rotate : number;
}

export interface Physics {

}

export interface UnityEndpoints {
    artwork : ArtworkEndpoints[];
}


export interface ArtworkEndpoints {
    transform : Transform;
    motifs : MotifEndpoints[];

}


export interface MotifEndpoints {
    transform : Transform;
    color : Color;
    motion : Motion;
}

const schema = {
    "endpoints": [
        {
            "id": "unique_id",
            "path": "endpoint_path",
            "label": "endpoint_label",
            "type": "type_of_data",
            "children": [
                {
                    "id": "unique_id",
                    "path": "endpoint_path",
                    "label": "endpoint_label",
                    "type": "type_of_data",
                    "children": []
                }
            ]
        }
    ]
}

export type EndpointType = "float" | "bang" | "int" | "string" | "bool" | "variable" | null;

export interface IEndpoint {
    id: string;
    directory: string;
    label: string;
    type: EndpointType;
    children: IEndpoint[];
  }
  
interface IEndpointSchema {
    endpoints: IEndpoint[];
}
  


// Ideas section:
// - it would be nice if all the pieces could individually scale in 
//   a motion that was wavy, in response to bangs to an endpoint

// with these basic interfaces (ignore them for implementation, I was just using them to help me conceptualize), can you create an example IEndpoint in json that has the given example endpoints in JSON? 