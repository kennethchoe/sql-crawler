import moment from "moment";

export function toLocalString(val: string): string {
  if (val) {
    const result = moment(val + " Z").format("llll");
    return result;
  }

  return "";
}
