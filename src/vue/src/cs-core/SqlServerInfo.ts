import { sqlServerInfoPublic } from './SqlServerInfoPublic';

export class sqlServerInfo extends sqlServerInfoPublic {
    public dataSource: string | null = null;
    public useIntegratedSecurity: boolean | null = false;
    public sqlUsername: string | null = null;
    public sqlPassword: string | null = null;
}
