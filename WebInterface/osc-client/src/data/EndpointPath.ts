
export class EndpointPath {
    components : Map<number, string>;
    constructor() {
        this.components = new Map<number, string>();
    }

    setAt(index: number, value: string) {
        this.components.set(index, value);
    }

    getAt(index: number) {
        return this.components.get(index);
    }

    get() {
        // sort by key 
        const sorted = Array.from(this.components.entries()).sort();
        const values = sorted.map(entry => entry[1]);
        return values.join("/");
    }

}
