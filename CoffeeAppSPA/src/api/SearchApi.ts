import axios, { AxiosResponse } from 'axios';

export class SearchApi {
  baseUrl: string;
  constructor(baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  async searchCoffees(query: string, topResults: number = 10): Promise<AxiosResponse> {
    const url = `${this.baseUrl}/api/search/coffees?query=${query}&topResults=${topResults}`;
    return await axios.get(url);
  }

  async searchCoffeeShops(query: string, topResults: number = 10): Promise<AxiosResponse> {
    const url = `${this.baseUrl}/api/search/coffeeshops?query=${query}&topResults=${topResults}`;
    return await axios.get(url);
  }

  async searchRoasters(query: string, topResults: number = 10): Promise<AxiosResponse> {
    const url = `${this.baseUrl}/api/search/roasters?query=${query}&topResults=${topResults}`;
    return await axios.get(url);
  }

  async searchAll(query: string, topResults: number = 10): Promise<AxiosResponse> {
    const url = `${this.baseUrl}/api/search/all?query=${query}&topResults=${topResults}`;
    return await axios.get(url);
  }
}